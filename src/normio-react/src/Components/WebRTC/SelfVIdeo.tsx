import React, {createRef, useEffect, useState} from "react";

const SelfVideo: React.FC = props => {
    const [stream, setStream] = useState<MediaStream | null>(null);
    const videoRef = createRef<HTMLVideoElement>();
    
    useEffect(() => {
        navigator.mediaDevices
            .getUserMedia({ video: true })
            .then(mediaStream => {
                setStream(mediaStream)
            })
            .catch(err => {
                console.error("Failed to find a media device", err)
            })
    }, [videoRef.current])
    
    useEffect(() => {
        if (videoRef.current) {
            videoRef.current.srcObject = stream
        }
    }, [stream])
    
    
    return (
        <div>
            <video ref={videoRef} autoPlay playsInline style={{ border: "1px solid black", width: "15vw" }} />
        </div>
    )
}

export default SelfVideo
