import React, {useCallback, useMemo, useState} from 'react'
import ExamInfo from '../ExamInfo/ExamInfo';
import {useForm} from "react-hook-form";
import axios from 'axios';
import {config} from "../../config/development";

type FindExamResult = "initial" | "ok" | "bad_request" | "not_found"

const FindExam: React.FC = props => {
    const [exam, setExam] = useState<any>(null)
    const [findResult, setFindResult] = useState<FindExamResult>("initial")
    const { register, handleSubmit } = useForm()
    const onSubmit = useCallback(async (data, e) => {
        e.preventDefault()
        try {
            const response = await axios.get(config.backendUrl + '/exam', {
                params: {
                    examId: data.examId,
                }
            })
            setFindResult("ok")
            setExam(response.data)
        } catch (error) {
            if (error.response.status === 400) {
                setFindResult("bad_request")
            }
            if (error.response.status === 404) {
                setFindResult("not_found")
            }
        } finally {
        }
    }, []);
    
    const interactiveBtn = useCallback(() => {
        switch (findResult) {
            case "initial":
                return ["primary", "시험을 찾습니다"]
            case "ok":
                return ["success", "시험을 찾았습니다"]
            case "bad_request":
                return ["warning", "시험 ID를 잘못 입력하셨어요"]
            case "not_found":
                return ["danger", "시험이 존재하지 않습니다"]
        }
    }, [findResult])
    const [btnClass, btnValue] = useMemo(() => interactiveBtn(), [interactiveBtn])
    
    const Centered:React.FC = props => {
        return (
            <div className={"row my-2"}>
                <div className="col-1 col-xs-2 col-md-3"></div>
                <div className="col-10 col-xs-8 col-md-6">
                    {props.children}
                </div>
                <div className="col-1 col-xs-2 col-md-3"></div>
            </div>
        )
    }
    
    return (
        <React.Fragment>
            <div className="container">
                    <Centered>
                        <div className={"card shadow-lg p-3"}>
                            <form onSubmit={handleSubmit(onSubmit)}>
                                <div className="form-group">
                                    <label>시험 ID를 입력하세요</label>
                                    <input 
                                        name={"examId"}
                                        ref={register}
                                        className="form-control"
                                        type={"text"}
                                        placeholder={"Exam Id"}
                                    />
                                </div>
                                <div className="form-group">
                                    <label>암호를 입력하세요</label>
                                    <input 
                                        name={"password"}
                                        ref={register}
                                        className="form-control"
                                        type={"password"}
                                        placeholder={"Exam Password"}
                                    />
                                </div>
                                <div className="form-group">
                                    <input 
                                        className={`from-control btn btn-${btnClass} btn-block`}
                                        type={"submit"} 
                                        value={btnValue}
                                    />
                                </div>
                            </form>
                        </div>
                    </Centered>
                { exam ?
                    <Centered>
                        <ExamInfo exam={exam} />
                    </Centered> : null
                }
            </div>
        </React.Fragment>
    )
}

export default FindExam