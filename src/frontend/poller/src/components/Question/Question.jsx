import React from 'react';

export class Question extends React.Component {
  render() {
    return  <div className="question">
              <p>{this.props.question.question}</p>
              <textarea></textarea>
            </div>;
  }
}