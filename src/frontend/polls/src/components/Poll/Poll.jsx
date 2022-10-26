import React from 'react';
import { NavLink, useParams } from 'react-router-dom';
import './style/poll.scss';

function Poll() {
	const { id } = useParams();
	return (
		<ul className="navigationLinksList">
			Poll  #{id}
			<NavLink key="home" to={'/'} className="navigationLinksItem">
				Home
			</NavLink>
		</ul>
	);
};

export default Poll;
