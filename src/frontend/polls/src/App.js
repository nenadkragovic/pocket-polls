import React, { Suspense } from 'react';
import { BrowserRouter } from 'react-router-dom';
import Footer from './components/Footer/Footer';
import Header from './components/Header/Header';
import Navigation from './components/Routes/Routes';
import './App.css';

function App() {
  return (
    <BrowserRouter>
			<Header />
			<Suspense fallback={<div style={{ flex: 1 }}>Loading...</div>}>
				<Navigation />
			</Suspense>
			{/* <Footer /> */}
		</BrowserRouter>
  );
}

export default App;
