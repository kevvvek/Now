import React, { useState } from 'react';
import { Form, Button, Alert, Card } from 'react-bootstrap';

const MusicSheet: React.FC = () => {
    const [youtubeUrl, setYoutubeUrl] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [jobId, setJobId] = useState<string | null>(null);
    const [generationStatus, setGenerationStatus] = useState<any>(null);
    const [audioTestInfo, setAudioTestInfo] = useState<any>(null);

    const handleGenerate = async () => {
        try {
            setIsLoading(true);
            setError(null);
            setAudioTestInfo(null);
            
            const response = await fetch('http://localhost:5257/api/PianoSheetGenerator/generate', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ youtubeUrl }),
            });

            if (!response.ok) {
                throw new Error('Failed to start generation process');
            }

            const data = await response.json();
            setJobId(data.jobId);
            
            // Start polling for status
            pollGenerationStatus(data.jobId);
        } catch (err) {
            setError(err instanceof Error ? err.message : 'An error occurred');
        } finally {
            setIsLoading(false);
        }
    };

    const pollGenerationStatus = async (jobId: string) => {
        try {
            const response = await fetch(`http://localhost:5257/api/PianoSheetGenerator/status/${jobId}`);
            if (!response.ok) {
                throw new Error('Failed to fetch status');
            }

            const status = await response.json();
            setGenerationStatus(status);

            if (status.state === 'Completed') {
                // Get test information about the audio file
                const filename = jobId.split('\\').pop(); // Get filename from path
                const testResponse = await fetch(`http://localhost:5257/api/PianoSheetGenerator/test/${filename}`);
                if (testResponse.ok) {
                    const testInfo = await testResponse.json();
                    setAudioTestInfo(testInfo);
                }
            }

            if (status.state !== 'Completed' && status.state !== 'Failed') {
                // Continue polling every 2 seconds
                setTimeout(() => pollGenerationStatus(jobId), 2000);
            }
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Failed to check status');
        }
    };

    return (
        <Card className="mt-4">
            <Card.Body>
                <Card.Title>Piano Sheet Music Generator</Card.Title>
                <Form>
                    <Form.Group className="mb-3" controlId="youtubeUrl">
                        <Form.Label>YouTube URL</Form.Label>
                        <Form.Control
                            type="text"
                            placeholder="Enter YouTube video URL"
                            value={youtubeUrl}
                            onChange={(e) => setYoutubeUrl(e.target.value)}
                        />
                        <Form.Text className="text-muted">
                            Paste the URL of the YouTube video you want to generate sheet music from
                        </Form.Text>
                    </Form.Group>

                    <Button 
                        variant="primary" 
                        onClick={handleGenerate}
                        disabled={isLoading || !youtubeUrl}
                    >
                        {isLoading ? 'Generating...' : 'Generate Sheet Music'}
                    </Button>
                </Form>

                {error && (
                    <Alert variant="danger" className="mt-3">
                        {error}
                    </Alert>
                )}

                {generationStatus && (
                    <Alert variant="info" className="mt-3">
                        <div>Status: {generationStatus.state}</div>
                        {generationStatus.progress > 0 && (
                            <div>Progress: {generationStatus.progress}%</div>
                        )}
                        {generationStatus.resultUrl && (
                            <div className="mt-3">
                                <h5>Audio Preview:</h5>
                                <audio controls className="w-100 mt-2">
                                    <source src={`http://localhost:5257${generationStatus.resultUrl}`} type="audio/mp4" />
                                    Your browser does not support the audio element.
                                </audio>
                            </div>
                        )}
                    </Alert>
                )}

                {audioTestInfo && (
                    <Alert variant="success" className="mt-3">
                        <h5>Audio File Information:</h5>
                        <pre style={{ whiteSpace: 'pre-wrap' }}>
                            {JSON.stringify(audioTestInfo, null, 2)}
                        </pre>
                    </Alert>
                )}
            </Card.Body>
        </Card>
    );
};

export default MusicSheet; 