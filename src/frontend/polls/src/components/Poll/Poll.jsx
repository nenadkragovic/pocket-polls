import React from 'react';
import { NavLink, useParams } from 'react-router-dom';
import * as style from './style/poll.scss';
import * as http from '../../scripts/http';
import Container from '@mui/material/Container';

function Poll() {
	const { id } = useParams();
	return (
		<Container style={style} className="container">
			Poll  #{id}
			<NavLink key="home" to={'/'} className="navigationLinksItem">
				Home
			</NavLink>
		</Container>
	);
};

export default Poll;
