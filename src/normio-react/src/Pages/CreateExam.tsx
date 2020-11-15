import React, {useCallback, useState} from "react";
import CreateExamCard from "../Components/CreateExam";

const CreateExamPage: React.FC = props => {
    const [examCreated, setExamCreated] = useState(false);
    
    const handleCreateExam = useCallback(() => {
        setExamCreated(true)
    }, [])
 
    return (
        <div className={"container"}>
            <div className={"row"}>
                { !examCreated ? <div className={"col-3 my-5"}></div> : null}
                <div className={"col"}>
                    <div className={"row"}>
                        <div className={"col my-3"}>
                            <div className={"card shadow-lg"}>
                                <div className={"card-header font-weight-bold text-center"}>
                                    호스트 계정으로 로그인하세요
                                </div>
                                <div className={"card-text p-3"}>
                                    <form>
                                        <div className={"form-group"}>
                                            <input className={"form-control"} placeholder={"Email"} />
                                        </div>
                                        <div className={"form-group"}>
                                            <input className={"form-control"} placeholder={"Password"} />
                                        </div>
                                        <input type="submit" className={`btn btn-primary btn-block`} value={"로그인"} />
                                    </form>
                                </div>
                                <div className={"text-center"}>
                                    <p className={"font-weight-light"}> 비밀번호를 잊으셨나요? </p>
                                    <p className={"font-weight-light"}> 호스트 계정이 없으신가요? </p>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className={"row"}>
                        <div className={"col my-3"}>
                            <CreateExamCard handleSubmit={handleCreateExam} />
                        </div>
                    </div>
                </div>
                    { examCreated ?
                        <div className={"col my-3"}>
                        <div className={"card shadow-lg"}>
                            <div className={"card-header font-weight-bold text-center"}>
                                이제 무엇을 해야 하나요?
                            </div>
                            <div className={"card-body"}>
                                <div className={"card-text text-center p-2"}>
                                    <h5> 시험 운영을 도와줄 다른 호스트들을 등록하세요. </h5>
                                    <p className={"font-weight-light"}> 다른 호스트들은 시험 정보에 접근할 수 있지만, 시험을 삭제할 수 없습니다. </p>
                                    <button className={"btn btn-light btn-block"}>네, 지금 등록할게요.</button>
                                </div>
                                <div className={"card-text text-center  p-2"}>
                                    <h5> 시험에 참여할 학생들을 등록하세요. </h5>
                                    <p className={"font-weight-light"}> 등록에 필요한 ID와 암호를 생성합니다. </p>
                                    <button className={"btn btn-light btn-block"}>네, 지금 등록할게요.</button>
                                </div>
                                <div className={"card-text text-center p-2"}>
                                    <h5> 앗! 뭔가 잘못 설정하셨나요? </h5>
                                    <p className={"font-weight-light"}> 안심하세요. 지금 바로 수정할 수 있습니다. </p>
                                    <button className={"btn btn-light btn-block"}>네, 수정하고 싶습니다.</button>
                                </div>
                                <div className={"card-text text-center p-2"}>
                                    <h5> 지금 바쁘신가요? </h5>
                                    <p className={"font-weight-light"}> 나중에 언제든지 돌아오세요. <br /> 등록된 계정에서 언제든지 다시 수정할 수 있습니다. </p>
                                    <button className={"btn btn-light btn-block"}>네, 그럴게요.</button>
                                </div>
                            </div>
                        </div>
                    </div> : <div className={"col-3 my-5"} />}
            </div>
        </div>
    )
}

export default CreateExamPage