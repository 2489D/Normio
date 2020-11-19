import React, {createRef, useCallback, useEffect, useRef, useState} from "react";

const SelfVideo: React.FC = props => {
    const [stream, setStream] = useState<MediaStream | null>(null);
    const videoRef = createRef<HTMLVideoElement>();
    const localPeerConn = useRef<RTCPeerConnection>()
    
    useEffect(() => {
        // set video device
        navigator.mediaDevices
            .getUserMedia({ video: true })
            .then(mediaStream => {
                setStream(mediaStream)
            })
            .catch(err => {
                console.error("Failed to find a media device", err)
            })
        
        const handleConnection = event => {
            const peerConn = event.target;
            const candid = event.candidate;
            
            if (candid) {
                const newIceCandidate = new RTCIceCandidate(candid);
                const otherPeer = getOtherPeer(peerConn)
                
                otherPeer.addIceCandidate(newIceCandidate)
                    .then(() => {
                        handleConnectionSuccess(peerConn);
                    }).catch(error => {
                        handleConnectionFailure(peerConn, error);
                })
            }
            console.debug(`${getPeerName(peerConn)} ICE candidate:\n` +
                `${event.candidate.candidate}.`);
        }
        
        const handleConnectionChange = event => {
            
        }
        localPeerConn.current = new RTCPeerConnection(/* server */);
        localPeerConn.current.addEventListener('icecandidate', handleConnection)
        localPeerConn.current.addEventListener('iceconnectionstatechange', handleConnectionChange)
        
        localPeerConn.current.createOffer(offerOptions)
            .then(createdOffer).catch(setSessionDescriptionError)
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
