import React, { useState, useEffect } from 'react'

type CurrentTimeProps = {
    variant?: string
}

const CurrentTime: React.FC<CurrentTimeProps> = props => {
    const [currentTime, setCurrentTime] = useState(new Date().toLocaleTimeString());

    useEffect(() => {
        const ticker = setInterval(() => {
            setCurrentTime(new Date().toLocaleTimeString())
        }, 1000)
        return () => {
            clearInterval(ticker)
        }
    }, [])

    return (
        <div className={props.variant}>
            {currentTime}
        </div>
    )
}

export default CurrentTime