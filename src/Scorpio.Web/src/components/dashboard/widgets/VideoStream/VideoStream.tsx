import React, {Component} from 'react';

class VideoStream extends Component {
  render() {
    return (
      <div>
        <video style={{width: "100%", height: "84vh", zIndex: 1, backgroundColor: "black"}} controls>
          <source src={`${process.env.PUBLIC_URL}/testresources/RickRoll.mp4`} type="video/mp4"/>
          Your browser does not support the video tag.
        </video>
      </div>
    );
  }
}

export default VideoStream;
