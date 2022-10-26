import React from 'react';
import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
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

const Home = () => {

	const [data, setData] = useState({
		items: [],
		offset: 0,
		limit: 5,
		totalRecords: 0
	  });

	useEffect(() => {
		fetchData(1);
	},[]);

	const fetchData = async (page, searchParam = '') => {
		let offset = data.limit * (page-1);
		let result = await http.httpRequest("polls?offset=" + offset + "&limit=" + data.limit + "&searchParam=" + searchParam, 'GET', null);
		if (result.status !== 200 || result.data.totalRecords < 1)
			return;

		setData({
			...data,
			items: result.data.records,
			offset: offset,
			totalRecords: result.data.totalRecords
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

	return (
		<Container style={style} className="container">
			<Autocomplete
				className='search'
				id="free-solo-demo"
				freeSolo
				options={[]}
				renderInput={(params) => <TextField {...params} label="Search" onChange={searchPolls} />}
			/>
			<List style={{maxHeight: '100%', overflow: 'auto'}}>
				{data.items.length > 0 ? 
					data.items.map((item) => (
					<Card className="poll-thumb" key={item.id}>
						<CardContent>
							<Typography variant="h5" component="div">
								({item.id}){item.name}
							</Typography>
							<Typography variant="body2">
								{item.description}
							</Typography>
						</CardContent>
						<CardActions>
						<Button size="small">
						<Link to={{pathname: '/poll/' + item.id}}>
										Answer
						</Link>
						</Button>
						</CardActions>
					</Card>
					)): <ListItem className="no-polls-message">No Polls found.</ListItem>}
			</List>
			{data.items.length > 0 ?
				<Pagination
				className='pagination'
				count={data.totalRecords/data.limit}
				size="large"
				onChange={handleChange}
				/>: null}
			
		</Container>
	  );
}

export default Home;
