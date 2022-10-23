import React from 'react';
import { NavLink } from 'react-router-dom';
import './style/poll.scss';

const Poll = () => {
	console.log('home mounted');
	return (
		<ul className="navigationLinksList">
			Poll component
			<NavLink key="home" exact to={'/'} className="navigationLinksItem">
				Home
			</NavLink>
		</ul>
	);
};

export default Poll;
