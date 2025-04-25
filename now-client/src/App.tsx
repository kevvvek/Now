import React, { useState, useEffect } from 'react';
import { Container, Nav, Navbar, Tab, Tabs } from 'react-bootstrap';
import './App.css';
import MusicSheet from './components/MusicSheet';

function App() {
  const [activeTab, setActiveTab] = useState('home');

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
        <Tabs
          activeKey={activeTab}
          onSelect={(k) => setActiveTab(k || 'home')}
          className="mb-3"
        >
          <Tab eventKey="home" title="Home">
            <h2>Welcome to Now Project</h2>
            <p>Select a module from the tabs above to get started.</p>
          </Tab>
          <Tab eventKey="musicsheet" title="Music Sheet Generator">
            <MusicSheet />
          </Tab>
          {/* Add more module tabs here */}
        </Tabs>
      </Container>
    </div>
  );
}

export default App;
