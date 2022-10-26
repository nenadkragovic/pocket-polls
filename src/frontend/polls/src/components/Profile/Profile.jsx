import React from 'react';
import { NavLink, useParams } from 'react-router-dom';
import './style/profile.scss';

function Profile() {
	return (
		<ul className="navigationLinksList">
			Profile
			<NavLink key="home" to={'/'} className="navigationLinksItem">
				Home
			</NavLink>
		</ul>
	);
};

export default Profile;
