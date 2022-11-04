import React from 'react';
import { useState, useEffect } from 'react';
import { NavLink, useParams } from 'react-router-dom';
import  * as style from './style/answers.scss';
import * as http from '../../scripts/http';
import { useNavigate } from 'react-router-dom';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import { Container } from '@mui/material';
import Button from '@mui/material/Button';
import DeleteForeverIcon from '@mui/icons-material/DeleteForever';
import * as validation from '../../scripts/validationHelper';
import Snackbar from '@mui/material/Snackbar';
import MuiAlert from '@mui/material/Alert';

function Answers() {
	const navigate = useNavigate();

	const [data, setData] = useState({
		items: [],
		offset: 0,
		limit: 5,
		totalRecords: 0,
		requestInProgress: false
	  });

	useEffect(() => {
		fetchData(1);
	},[]);

	const [validationData, setValidationData] = useState({
		open: false,
		message: '',
		severity: 'info'
	});

	const handleCloseValidationMessage = () => {
		setValidationData({
			open: false,
			message: '',
			severity: 'info'
		})
	}

	const fetchData = async (page, searchParam = '') => {
		setData({
			...data,
			requestInProgress: true
		});
		let offset = data.limit * (page-1);
			await http.request("polls?getForUser=true&offset=" + offset + "&limit=" + data.limit + "&searchParam=" + searchParam, 'GET', null)
				.then(result => {
					setData({
						...data,
						items: result.data.records,
						offset: offset,
						totalRecords: result.data.totalRecords,
						requestInProgress: false
					});
				}).catch(err => {
					setData({
						...data,
						requestInProgress: false
					});
				});

	};

	const searchPolls = async (event) => {
		setData({
			...data,
			items: [],
			totalRecords: 0,
			offset: 0
		});
		await fetchData(1, event.target.value);
	}

	const handleChange = async (e, p) => {
		await fetchData(p);
	}

	const openPoll = (url) => {
		navigate(url);
	}

	const deletePoll = async (pollId) => {
		await http.request(`polls/${pollId}`, 'DELETE', null)
		.then(result => {
			setValidationData({
				open: true,
				message: 'Poll deleted successfully!',
				severity: 'success'
			})
			var items = data.items.filter(function(value, index, arr){
				return value.id !== pollId;
			});
			setData({
				...data,
				items: items
			});
		}).catch(err => {
			var message = validation.getValidationMessage(err.response.data);

			setValidationData({
				open: true,
				message: message,
				severity: 'error'
			})
		});
	}

	return (
		<>
			<Container style={style}>
				<TableContainer component={Paper} style={{marginTop: '1rem'}}>
				<Table sx={{ minWidth: '100%' }} aria-label="simple table">
					<TableHead>
					<TableRow>
						<TableCell align="left">Id</TableCell>
						<TableCell>Name</TableCell>
						<TableCell align="right">Action</TableCell>
					</TableRow>
					</TableHead>
					<TableBody>
					{data.items.map((row) => (
						<TableRow
						key={row.id}
						sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
						>
						<TableCell component="th" align="left" scope="row">
							{row.id}
						</TableCell>
						<TableCell>{row.name}</TableCell>
						<TableCell align="right">
							<Button variant="outlined" onClick={()=> deletePoll(row.id)}><DeleteForeverIcon></DeleteForeverIcon></Button>
						</TableCell>
						</TableRow>
					))}
					</TableBody>
				</Table>
				</TableContainer>
			</Container>
			<Snackbar
				className='validation'
				open={validationData.open}
				autoHideDuration={6000}
				onClose={handleCloseValidationMessage}>
					<MuiAlert severity={validationData.severity} elevation={6} variant="filled" onClose={handleCloseValidationMessage}>
						{validationData.message}
					</MuiAlert>
			</Snackbar>
		</>
	);
};

export default Answers;
