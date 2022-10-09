import React from 'react';
import { Poll } from '../Poll/Poll.jsx'
import * as objects from '../../scripts/objects'
import store from '../../redux/store'

export class Home extends React.Component {

  constructor(props) {
    super(props);
    this.polls = [];
    let poll1 = new objects.Poll();
    poll1.name = 'Prvi poll';
    poll1.addQuestion(new objects.YesNoQuestion(0, 'Da li ste punoletni'));
    this.polls.push(poll1);
  }

  componentDidMount() {
    console.log('Home loaded')


    // store.dispatch('SET_POLLS', this.polls);
  }

  componentDidUpdate() {
  }

  render() {
    return  <div >
              Home Components

              {this.polls.map((poll, i) => {
                 console.log("Entered poll: ", i);
                 // Return the element. Also pass key
                 return (<Poll poll={poll} />)
              })}
            </div>;
  }
}