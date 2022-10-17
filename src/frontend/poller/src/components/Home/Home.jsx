import React from 'react';
import { Poll } from '../Poll/Poll.jsx'
import * as objects from '../../scripts/objects'
import store from '../../redux/store'
import './Home.css'

export class Home extends React.Component {

  constructor(props) {
    super(props);
    this.polls = [];
    let poll1 = new objects.Poll();
    poll1.name = 'Prvi poll';
    poll1.description = '';
    poll1.image = 'https://zoomue.rs/wp-content/uploads/2022/07/Popis-ILUSTRACIJA.jpg';
    poll1.addQuestion(new objects.YesNoQuestion(0, 'Da li ste punoletni'));
    poll1.addQuestion(new objects.TextQuestion(1, 'Godina rodjenja?'));
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

              <div className="pin_container">
                {this.polls.map((poll, i) => {
                 console.log("Entered poll: ", i);
                 // Return the element. Also pass key
                 return (
                  <div className="card card_large" key={poll.name}>{poll.name}
                    <img src={poll.image}></img>
                    <button >Go!</button>
                    <Poll hidden={true} key={i} poll={poll}></Poll>
                  </div>
                   )
                })}
              </div>
            </div>;
  }
}