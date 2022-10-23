import React from 'react';
import { NavLink } from 'react-router-dom';
import './style/home.scss';

const Home = () => {
	console.log('home mounted');
	return (
		<ul className="navigationLinksList">
			Home component
			<NavLink key="pol" exact to={'/poll'} className="navigationLinksItem">
				Poll
			</NavLink>
		</ul>
	);
};

export default Home;
