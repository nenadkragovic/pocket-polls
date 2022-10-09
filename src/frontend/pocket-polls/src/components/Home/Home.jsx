import React from 'react';
import Poll from '../Poll/Poll.jsx'

export class Home extends React.Component {
  render() {
    return  <div >
              Home Component
              <Poll Name="Poll example"/>
            </div>;
  }
}