import React from 'react';
import { Routes, Route } from 'react-router-dom';
import Home from '../Home/Home';
import Poll from '../Poll/Poll';

const Navigation = () => {
	return (
		<Routes>
			<Route path='/' element={<Home/>}/>
			<Route path='/poll/:id' element={<Poll/>} />
		</Routes>
	);
};

export default Navigation;
