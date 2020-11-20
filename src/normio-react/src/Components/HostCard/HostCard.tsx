import React from "react";
import {Normio} from "../../Context/ExamContext";


const HostLive: React.FC<{host: Normio.Host}> = ({ host, ...props }) => {
    return (
        <div className={"card"}>
            <div className={"card-body"}>
                <h5 className={"card-title"}>
                    {host.name}
                </h5>
                <h6 className={"card-subtitle text-muted"}>
                    시험 감독
                </h6>
            </div>
        </div>
    )
}
export default HostLive