import React from 'react';
import { useEffect, useState }from 'react';
import { Link } from 'react-router-dom';
import InfiniteScroll from 'react-infinite-scroll-component';
import * as style from './style/home.scss';
import * as http from '../../scripts/http';

const Home = () => {

	const [data, setData] = useState({
		items: [],
		offset: 0,
		limit: 10,
		searchParam: '',
		totalRecords: 0
	  });

	useEffect(() => {
		fetchMoreData();
	},[]);

	const fetchMoreData = async () => {
		let result =  await http.httpRequest("polls", 'GET', {
			offset: data.offset,
			limit: data.limit, 
			searchParam: data.searchParam
		});

		setData(
			{...data,
			items: [... data.items, ... result.data.records],
			totalRecords: result.data.totalRecords,
			offset: data.offset + data.limit
		});

		console.log(data);
	};
  
	const refresh = async () => {
		setData(
			{
			...data,
			items: [],
			totalRecords: 0,
			offset: 0
		});
		await fetchMoreData();
	}

	return (
		<div className='container' style={style}>
		  <InfiniteScroll
		    className='pollThumb'
			dataLength={data.limit} //This is important field to render the next data
			next={fetchMoreData}
			hasMore={true}
			loader={<h4>Loading...</h4>}
			endMessage={
				<p style={{ textAlign: 'center' }}>
				<b>Yay! You have seen it all</b>
				</p>
			}
			// below props only if you need pull down functionality
			refreshFunction={refresh}
			pullDownToRefresh
			pullDownToRefreshThreshold={50}
			pullDownToRefreshContent={
				<h3 style={{ textAlign: 'center' }}>&#8595; Pull down to refresh</h3>
			}
			releaseToRefreshContent={
				<h3 style={{ textAlign: 'center' }}>&#8593; Release to refresh</h3>
			}
			>

			{data.items.length > 0 ? data.items.map((item) => (
			  	<Link key={item.id} to={{pathname: '/poll/' + item.id}} className="poll">
					<div className='thumb'>Poll #{item.id}</div>
				</Link>
			)): null}

			</InfiniteScroll>
		</div>
	  );
}

export default Home;
