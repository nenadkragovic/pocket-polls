import React from 'react';

export class Poll extends React.Component {
  render() {
    return  <div >
              <h>{this.props.Name}</h>
            </div>;
  }
}