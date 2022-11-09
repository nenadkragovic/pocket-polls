import React from 'react';
import { useEffect, useState } from 'react';
import TextField from '@mui/material/TextField';
import Autocomplete from '@mui/material/Autocomplete';
import * as style from './style/home.scss';
import * as http from '../../scripts/http';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import Container from '@mui/material/Container';
import Pagination from '@mui/material/Pagination';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import Skeleton from '@mui/material/Skeleton';
import { useNavigate } from 'react-router-dom';
import * as serviceWorkerRegistration from '../../serviceWorkerRegistration';

const Home = () => {
	const navigate = useNavigate();

	const [data, setData] = useState({
		items: [],
		limit: 5,
		totalRecords: 0,
		requestInProgress: false
	  });

	useEffect(() => {
		fetchData(1);
		let userId = localStorage.getItem('user-id');
		// If you want your app to work offline and load faster, you can change
		// unregister() to register() below. Note this comes with some pitfalls.
		// Learn more about service workers: https://cra.link/PWA
		serviceWorkerRegistration.setUserId(userId);
		serviceWorkerRegistration.register();
		serviceWorkerRegistration.requestNotificationPermission();
	}, [null]);

	const fetchData = async (page, searchParam = '') => {
		setData({
			...data,
			requestInProgress: true
		});
		let offset = data.limit * (page-1);
			await http.request("polls?offset=" + offset + "&limit=" + data.limit + "&searchParam=" + searchParam, 'GET', null)
				.then(result => {
					if (result.status === 200)
						setData({
							...data,
							items: result.data.records,
							totalRecords: result.data.totalRecords,
							requestInProgress: false
						});
					else {
						setData({
							...data,
							items: [],
							totalRecords: 0,
							requestInProgress: false
						});
					}
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
			totalRecords: 0
		});
		await fetchData(1, event.target.value);
	}

	const handleChange = async (e, p) => {
		await fetchData(p);
	}

	const openPoll = (url) => {
		navigate(url);
	}

	return (
		<Container style={style} className="container">
			<Autocomplete
				className='search'
				id="free-solo-demo"
				freeSolo
				options={[]}
				renderInput={(params) => <TextField {...params} label="Search" onChange={searchPolls} />}
			/>
			<List className="poll-thumbs-list">
				{ data.requestInProgress ?
					<div>
						{[...Array(data.limit)].map((x, i) =>
							<div key={i} style={{height: '10rem', border: 0}} className="poll-thumb">
								<Skeleton animation="wave"  style={{height: '2rem'}}/>
								<Skeleton variant="rectangular" animation="wave" style={{height: '5rem'}}/>
								<Skeleton variant="rectangular" animation="wave"  style={{height: '2rem', width: '5rem'}}/>
							</div>
						)}
					</div> :
					data !== undefined && data.totalRecords > 0 ?
					data.items?.map((item) => (
					<Card sx={{ boxShadow: 3 }} className="poll-thumb" key={item.id}>
						<CardContent>
							<Typography variant="h5" component="div">
								{item.name}
								{/* (#{item.id}) */}
							</Typography>
							<Typography variant="body2">
								{item.description}
							</Typography>

						</CardContent>
						<CardActions className="card-actions">
							<Typography variant="p">
								{item.numberOfQuestions} questions.
							</Typography>
							<Button className="answer-poll-button" variant="outlined" size="small" onClick={() => openPoll('/poll/' + item.id)}>Answer</Button>
						</CardActions>
					</Card>
					)): <ListItem className="no-polls-message">No Polls found.</ListItem>}
			</List>

			{
				data.items.length > 0 ?
					<Pagination
					className='pagination'
					count={Math.ceil(data.totalRecords/data.limit)}
					size="large"
					onChange={handleChange}
					/> : null
			}
		</Container>
	  );
}

export default Home;
