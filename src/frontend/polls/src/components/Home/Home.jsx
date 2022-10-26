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

const Home = () => {
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

	const fetchData = async (page, searchParam = '') => {
		setData({
			...data,
			requestInProgress: true
		});
		let offset = data.limit * (page-1);
			await http.httpRequest("polls?offset=" + offset + "&limit=" + data.limit + "&searchParam=" + searchParam, 'GET', null)
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

	return (
		<Container style={style} className="container">
			<Autocomplete
				className='search'
				id="free-solo-demo"
				freeSolo
				options={[]}
				renderInput={(params) => <TextField {...params} label="Search" onChange={searchPolls} />}
			/>
			<List style={{maxHeight: '100%', overflow: 'auto'}} className="poll-thumbs-list">
				{ data.requestInProgress ?
					<div>
						{[...Array(data.limit)].map((x, i) =>
							<div style={{height: '10rem', border: 0}} className="poll-thumb">
								<Skeleton animation="wave"  style={{height: '2rem'}}/>
								<Skeleton variant="rectangular" animation="wave" style={{height: '5rem'}}/>
								<Skeleton variant="rectangular" animation="wave"  style={{height: '2rem', width: '5rem'}}/>
							</div>
						)}
					</div> :
					data.items.length > 0 ?
					data.items.map((item) => (
					<Card className="poll-thumb" key={item.id}>
						<CardContent>
							<Typography variant="h5" component="div">
								{item.name} (#{item.id})
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
					count={data.totalRecords/data.limit}
					size="large"
					onChange={handleChange}
					/> : null
			}
		</Container>
	  );
}

export default Home;
