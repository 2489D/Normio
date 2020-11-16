import React, {useCallback, useState} from "react";
import {useForm} from "react-hook-form";
import axios from 'axios';
import {config} from "../../config/development";

type CreateExamCardProps = {
    hostId?: string,
    handleSubmitCreateExam: () => void,
}

const CreateExamCard: React.FC<CreateExamCardProps> = ({ hostId, handleSubmitCreateExam }) => {
    const [created, setCreated] = useState(false)
    const { register, handleSubmit } = useForm()
 
    const onSubmit = useCallback(async data => {
        console.debug(data)
        const response = await axios.post(config.backendUrl + "/api/openExam", {
            title: data.title
        })
        console.debug(response)
        setCreated(true)
        handleSubmitCreateExam()
    }, [setCreated])

    return (
        <div className={"card shadow-lg"}>
            <div className={"card-header font-weight-bold"}>
                시험 구성하기
            </div>
            <div className={"card-text p-3"}>
                <form onSubmit={handleSubmit(onSubmit)}>
                    <div className={"form-group"}>
                        <input
                            name={"hostId"}
                            ref={register(
                                
                            )}
                            className={"form-control"}
                            placeholder={"Host ID"} 
                            defaultValue={hostId}
                        />
                    </div>
                    <div className={"form-group"}>
                        <input 
                            name={"title"}
                            ref={register}
                            className={"form-control"}
                            placeholder={"시험 명"}
                        />
                    </div>
                    <div className={"form-group"}>
                        <input 
                            name={"startDateTime"}
                            ref={register}
                            className={"form-control"}
                            placeholder={"시험 시작 시간"}
                        />
                    </div>
                    <input type="submit" className={`btn btn-${ created ? "success"  : "primary"} btn-block`} value={`${created ? "시험이 생성 되었습니다!"  : "시험을 생성합니다"}`} />
                </form>
            </div>
        </div>
    )
}

export default CreateExamCard