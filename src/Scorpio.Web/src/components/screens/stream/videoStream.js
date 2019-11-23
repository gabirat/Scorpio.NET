import React, { Component } from "react";

class VideoStream extends Component {
  render() {
    return (
      <div>
        <video style={{ width: "100%", height: "84vh", zIndex: 1, backgroundColor: "black" }} controls>
          <source type="video/youtube" src={"http://www.youtube.com/watch?v=nOEw9iiopwI"} />
          Your browser does not support the video tag.
        </video>
      </div>
    );
  }
}

export default VideoStream;
