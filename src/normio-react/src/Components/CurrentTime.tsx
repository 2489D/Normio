import React, { useState, useEffect } from 'react'

const CurrentTime: React.FC = () => {
    const [currentTime, setCurrentTime] = useState(new Date().toLocaleTimeString());

    useEffect(() => {
        setInterval(() => {
            setCurrentTime(new Date().toLocaleTimeString())
        }, 1000)
    }, [])

    return (
        <div>{currentTime} </div>
    )
}

export default CurrentTime