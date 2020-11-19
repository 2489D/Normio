import React, {createRef, useEffect, useState} from 'react';

const RemoteVideo: React.FC = props => {
    const [stream, setStream] = useState<MediaStream | null>(null);
    const conn = createRef<RTCPeerConnection>();
    const videoRef = createRef<HTMLVideoElement>();
    
    useEffect(() => {
        
        
        return () => {
            // release computing resources
            
        }
    }, [])
    
    return (
        <div>
            <video ref={videoRef} autoPlay playsInline style={{ border: "1px solid black", width: "15vw" }} />
        </div>
    )
}