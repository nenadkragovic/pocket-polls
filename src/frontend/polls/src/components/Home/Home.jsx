import React from 'react';
import { Link } from 'react-router-dom';
import InfiniteScroll from 'react-infinite-scroll-component';
import * as style from './style/home.scss';

class Home extends React.Component {
	state = {
	  items: Array.from({ length: 20 })
	};
  
	fetchMoreData = () => {
	  // a fake async api call like which sends
	  // 20 more records in 1.5 secs
	  setTimeout(() => {
		this.setState({
		  items: this.state.items.concat(Array.from({ length: 20 }))
		});
	  }, 1500);
	  // TODO: add style and fetch data from api
	};
  
	render() {
	  return (
		<div>
		  <ul className="navigationLinksList">
		  <InfiniteScroll
			dataLength={this.state.items.length}
			next={this.fetchMoreData}
			hasMore={true}
			loader={<h4>Loading...</h4>}
		  >
			{this.state.items.map((i, index) => (
			  <div style={style} key={index}>
				<Link key={"pol" + index} to={{pathname: '/poll/' + index}} className="poll">
					Poll #{index}
				</Link>
			  </div>
			))}
		  </InfiniteScroll>
		  </ul>
		</div>
	  );
	}
}

export default Home;
