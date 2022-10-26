import React from 'react';
import { NavLink, useParams } from 'react-router-dom';
import './style/createPoll.scss';

function CreatePoll() {
	return (
		<ul className="navigationLinksList">
			Create Poll
			<NavLink key="home" to={'/'} className="navigationLinksItem">
				Home
			</NavLink>
		</ul>
	);
};

export default CreatePoll;
