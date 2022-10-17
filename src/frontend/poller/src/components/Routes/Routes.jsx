import React from 'react';
import { Routes , Route } from 'react-router-dom';

const Routing = () => {
	const Home = React.lazy(() => import('../Home/Home'));
	const Poll = React.lazy(() => import('../Poll/Poll'));

	const routes = [
		{
			name: 'home',
			url: '/',
			component: <Home />,
		},
		{
			name: 'poll',
			url: '/poll',
			component: <Poll />,
		},
	];
	return (
		<Routes>
			{routes.map(({ name, url, component }) => {
					return (
						<Route
							exact
							path={url}
							key={`route_${name}`}
							render={props => {
								return <>{component}</>;
							}}
						/>
					);
				})}
		</Routes>
	);
};

export default Routing;
