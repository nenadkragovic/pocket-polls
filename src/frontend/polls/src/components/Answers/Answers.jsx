import React from 'react';
import { NavLink, useParams } from 'react-router-dom';
import './style/answers.scss';

function Answers() {
	return (
		<ul className="navigationLinksList">
			Answers
			<NavLink key="home" to={'/'} className="navigationLinksItem">
				Home
			</NavLink>
		</ul>
	);
};

export default Answers;
