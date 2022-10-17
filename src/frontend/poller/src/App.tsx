import React, { Suspense } from 'react';
import { BrowserRouter } from 'react-router-dom';
import './App.css';
import Routing from './components/Routes/Routes';

function App() {
  return (
    <div className="App">
     	<BrowserRouter>
			<Suspense fallback={<div style={{ flex: 1 }}>Loading...</div>}>
				<Routing />
			</Suspense>
		</BrowserRouter>
    </div>
  );
}

export default App;
