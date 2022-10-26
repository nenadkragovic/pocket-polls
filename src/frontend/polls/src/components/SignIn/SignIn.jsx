import React from 'react';
import { useState }from 'react';
import './style/signIn.scss';
import Box from '@mui/material/Box';
import Container from '@mui/material/Container';
import FormControl from '@mui/material/FormControl';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import InputAdornment from '@mui/material/InputAdornment';
import IconButton from '@mui/material/IconButton';
import Visibility from '@mui/icons-material/Visibility';
import VisibilityOff from '@mui/icons-material/VisibilityOff';
import FormLabel from '@mui/material/FormLabel';
import InputLabel from '@mui/material/InputLabel';
import OutlinedInput from '@mui/material/OutlinedInput';
import * as http from '../../scripts/http';
import { useNavigate } from 'react-router-dom';

const SignIn = (props) => {
	const navigate = useNavigate();

	let initData = {
		logInOrSignIn: true,
		showPassword: false,
		username: '',
		password: '',
		fullName: '',
		phoneNumber: '',
		email: '',
		address: ''
	  }
	const [data, setData] = useState(initData);

	const handleChange = (prop) => (event) => {
		setData({ ...data, [prop]: event.target.value });
	};

	const handleClickShowPassword = () => {
		setData({
			...data,
			showPassword: !data.showPassword,
		});
	};

	const constSwitchForms = () => {
		setData({
			...data,
			logInOrSignIn: !data.logInOrSignIn
		})
	}

	const logIn = async () => {
		await http.request('users/login', 'POST', {
				userName: data.username,
				password: data.password
			}).then(response => {
				if (response.status === 200 && response.data.token !== null)
				{
					localStorage.setItem('user-token', response.data.token);
					props.isLoggedAction(true);
					setData(initData);
					navigate('/');
				}
			}).catch(err => {

			})
	}

	const register = async () => {
		await http.request('users/register', 'POST', {
			userName: data.username,
			password: data.password,
			email: data.email,
			fullName: data.fullName,
			address: data.address
		}).then(response => {
			if (response.status === 201)
			{
				logIn();
			}
		}).catch(err => {

		})
	}

	return (
		<Container style={{textAlign: "center", maxWidth: '20rem'}} className="form-container">
			{
				data.logInOrSignIn ?
				<Box className="form-box">
					<Typography variant="h5" component="div" style={{ marginBottom: '2rem'}}>Log in</Typography>
					<FormControl className="form-control">
						<InputLabel htmlFor="outlined-adornment-password">Username</InputLabel>
						<OutlinedInput
							value={data.username}
							onChange={handleChange('username')}
							aria-describedby="outlined-weight-helper-text"
							label="Username"
						/>
					</FormControl>
					<FormControl className="form-control">
						<InputLabel htmlFor="outlined-adornment-password">Password</InputLabel>
          				<OutlinedInput
							id="outlined-adornment-password"
							type={data.showPassword ? 'text' : 'password'}
							value={data.password}
							onChange={handleChange('password')}
							endAdornment={
							<InputAdornment position="end">
								<IconButton
								aria-label="toggle password visibility"
								onClick={handleClickShowPassword}
								edge="end"
								>
								{data.showPassword ? <VisibilityOff /> : <Visibility />}
								</IconButton>
							</InputAdornment>
							}
							label="Password"/>
					</FormControl>
					<FormControl className="form-control">
						<Button variant="contained" onClick={logIn}>Log in</Button>
					</FormControl>
					<FormControl className="form-control">
						<FormLabel variant="contained" onClick={constSwitchForms}>Don't have account? Register.</FormLabel>
					</FormControl>
				</Box> :
				<Box className="form-box">
					<Typography variant="h5" component="div" style={{ marginBottom: '2rem'}}>Register</Typography>
					<FormControl className="form-control">
						<InputLabel htmlFor="outlined-adornment-password">Username</InputLabel>
						<OutlinedInput
							value={data.username}
							onChange={handleChange('username')}
							aria-describedby="outlined-weight-helper-text"
							label="Username"
						/>
					</FormControl>
					<FormControl className="form-control">
						<InputLabel htmlFor="outlined-adornment-password">Password</InputLabel>
          				<OutlinedInput
							id="outlined-adornment-password"
							type={data.showPassword ? 'text' : 'password'}
							value={data.password}
							onChange={handleChange('password')}
							endAdornment={
							<InputAdornment position="end">
								<IconButton
								aria-label="toggle password visibility"
								onClick={handleClickShowPassword}
								edge="end"
								>
								{data.showPassword ? <VisibilityOff /> : <Visibility />}
								</IconButton>
							</InputAdornment>
							}
							label="Password"/>
					</FormControl>
					<FormControl className="form-control">
						<InputLabel htmlFor="outlined-adornment-password">Email</InputLabel>
						<OutlinedInput
							value={data.email}
							onChange={handleChange('email')}
							aria-describedby="outlined-weight-helper-text"
							label="Email"
							type="email"
						/>
					</FormControl>
					<FormControl className="form-control">
						<InputLabel htmlFor="outlined-adornment-password">Full Name</InputLabel>
						<OutlinedInput
							value={data.fullName}
							onChange={handleChange('fullName')}
							aria-describedby="outlined-weight-helper-text"
							label="Full Name"
							type="text"
						/>
					</FormControl>
					<FormControl className="form-control">
						<InputLabel htmlFor="outlined-adornment-password">Address</InputLabel>
						<OutlinedInput
							value={data.address}
							onChange={handleChange('address')}
							aria-describedby="outlined-weight-helper-text"
							label="Address"
							type="text"
						/>
					</FormControl>
					<FormControl className="form-control">
						<Button variant="contained" onClick={register}>Register</Button>
					</FormControl>
					<FormControl className="form-control">
						<FormLabel variant="contained" onClick={constSwitchForms}>Already have account? Log in.</FormLabel>
					</FormControl>
				</Box>
			}
		</Container>
	);
};

export default SignIn;
