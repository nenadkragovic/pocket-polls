import { createStore, applyMiddleware, compose } from 'redux';
import { reducer } from './reducer'
import thunk from 'redux-thunk';

const middleware = [thunk];

const store = createStore(reducer, compose(applyMiddleware(...middleware)));

export default store;