import React from 'react';
import './style/footer.scss';
import { NavLink } from 'react-router-dom';
import { useLocation } from 'react-router-dom';

const Footer = () => {
	const location = useLocation();
	console.log(location);
	return (
		<div className="footerWrapper">
			footer
			{location.pathname === '/' ? null : (
				<NavLink className="footerLink backLink" exact to="/">
					back
				</NavLink>
			)}
		</div>
	);
};

export default Footer;
