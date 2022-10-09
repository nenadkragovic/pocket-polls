import React from 'react';
import { Question } from '../Question/Question';

export class Poll extends React.Component {
  render() {
    return  <div >
              <h>{this.props.poll.name}</h>
              {this.props.poll.questions.map((question, i) => {
                 console.log("Entered: ", i);
                 // Return the element. Also pass key
                 return (<Question question={question} />)
              })}
            </div>;
  }
}