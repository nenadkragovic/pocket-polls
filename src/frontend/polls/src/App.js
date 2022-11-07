import React, { Suspense, useEffect, useState } from 'react';
import { BrowserRouter } from 'react-router-dom';
import Menu from './components/Menu/Menu.jsx'
import { Routes, Route } from 'react-router-dom';
import Answers from './components/Answers/Answers';
import CreatePoll from './components/CreatePoll/CreatePoll';
import Home from './components/Home/Home';
import Poll from './components/Poll/Poll';
import Profile from './components/Profile/Profile';
import SignIn from './components/SignIn/SignIn';
import './App.css';

function App() {

  const [userLogged, setUserLogged] = useState(localStorage.getItem('user-token') != null);
  useEffect(() => {}, []);

  const isLoggedAction = (logged) => {
    setUserLogged(logged);
  };

  return (
    <BrowserRouter>
      {
        userLogged ?
          <>
            <Menu isLoggedAction={isLoggedAction} />
            <Suspense fallback={<div style={{ flex: 1 }}>Loading...</div>}>
            <Routes>
              <Route path='/' element={<Home/>}/>
              <Route path='/poll/:id' element={<Poll/>} />
              <Route path='/signin' element={<SignIn/>} />
              <Route path='/answers' element={<Answers/>} />
              <Route path='/createpoll' element={<CreatePoll/>} />
              <Route path='/profile' element={<Profile/>} />
            </Routes>
            </Suspense>
          </> : <SignIn isLoggedAction={isLoggedAction}/>
      }
    </BrowserRouter>
  );
}

export default App;
