import React, {createRef, useState} from 'react';

const RemoteVideo: React.FC = props => {
    const [stream, setStream] = useState<MediaStream | null>(null);
    const videoRef = createRef<HTMLVideoElement>();
    
    return (
        <div>
            <video ref={videoRef} autoPlay playsInline style={{ border: "1px solid black", width: "15vw" }} />
        </div>
    )
}