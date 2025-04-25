import React, { useState, useEffect } from 'react';
import { Container, Nav, Navbar, Row, Col, Card, Alert } from 'react-bootstrap';
import './App.css';

function App() {
  const [message, setMessage] = useState<string>('');
  const [error, setError] = useState<string>('');
  const [isLoading, setIsLoading] = useState<boolean>(true);

  useEffect(() => {
    console.log('Fetching message from backend...');
    fetch('http://localhost:5257/api/helloworld')
      .then(response => {
        console.log('Response received:', response);
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
      })
      .then(data => {
        console.log('Data received:', data);
        setMessage(data.message);
      })
      .catch(err => {
        console.error('Error fetching message:', err);
        setError('Failed to fetch message from server: ' + err.message);
      })
      .finally(() => {
        setIsLoading(false);
      });
  }, []);

  return (
    <div className="App">
      <Navbar bg="dark" variant="dark" expand="lg">
        <Container>
          <Navbar.Brand href="#home">Now Project</Navbar.Brand>
          <Navbar.Toggle aria-controls="basic-navbar-nav" />
          <Navbar.Collapse id="basic-navbar-nav">
            <Nav className="me-auto">
              <Nav.Link href="#home">Home</Nav.Link>
              <Nav.Link href="#features">Features</Nav.Link>
              <Nav.Link href="#about">About</Nav.Link>
            </Nav>
          </Navbar.Collapse>
        </Container>
      </Navbar>

      <Container className="mt-4">
        <Row>
          <Col md={8}>
            <Card>
              <Card.Body>
                <Card.Title>Welcome to Now Project</Card.Title>
                {isLoading && <Alert variant="info">Loading message from backend...</Alert>}
                {error && <Alert variant="danger">{error}</Alert>}
                {message && <Alert variant="success">{message}</Alert>}
                <Card.Text>
                  This is a full-stack application built with React and ASP.NET Core.
                  The frontend is styled with Bootstrap for a responsive and modern UI.
                </Card.Text>
              </Card.Body>
            </Card>
          </Col>
          <Col md={4}>
            <Card>
              <Card.Body>
                <Card.Title>Quick Links</Card.Title>
                <Nav className="flex-column">
                  <Nav.Link href="#dashboard">Dashboard</Nav.Link>
                  <Nav.Link href="#profile">Profile</Nav.Link>
                  <Nav.Link href="#settings">Settings</Nav.Link>
                </Nav>
              </Card.Body>
            </Card>
          </Col>
        </Row>
      </Container>
    </div>
  );
}

export default App;
