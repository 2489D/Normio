import React, {useCallback, useState} from "react";
import CreateExamCard from "../Components/CreateExam";
import HostLogin from "../Components/HostLogin";
import {nanoid} from 'nanoid';
import {useForm} from "react-hook-form";
import NormioApi from "../API";

const AddHostSection: React.FC<{ examId: string }> = ({examId}) => {
    const [hostId, setHostId] = useState("")
    const modalId = "addHostModal"
    const [added, setAdded] = useState(false)
    const {register, handleSubmit, reset} = useForm()

    const onSubmit = useCallback(async ({name}) => {
        const {data} = await NormioApi.addHost(examId, name)
        setHostId(data[0].Fields.host.id)
        setAdded(true)
        reset()
    }, [examId, setAdded])
    return (
        <div className={"card-text text-center p-2"}>
            <h5> 시험 운영을 도와줄 다른 호스트들을 등록하세요. </h5>
            <p className={"font-weight-light"}> 다른 호스트들은 시험 정보에 접근할 수 있지만, 시험을 삭제할 수 없습니다. </p>
            <button className={"btn btn-light btn-block"} data-toggle="modal" data-target={`#${modalId}`}>네, 지금 등록할게요.
            </button>
            <div className="modal fade" id={modalId} tabIndex={-1} role="dialog" aria-labelledby="addHostModal"
                 aria-hidden="true">
                <div className="modal-dialog" role="document">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h5 className="modal-title" id="exampleModalLabel">호스트를 추가합니다</h5>
                            <button type="button" className="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <form onSubmit={handleSubmit(onSubmit)}>
                            <div className="modal-body">
                                <div className={"form-group"}>
                                    <input className={"form-control"} type="text" name={"name"} placeholder={"호스트 이름"}
                                           ref={register}/>
                                </div>
                                {hostId ?? <div className={"text-center"}>
                                    <p>생성된 호스트 아이디입니다.</p>
                                    <div className={"font-weight-light"}>
                                        {hostId}
                                    </div>
                                </div>}
                            </div>
                            <div className="modal-footer">
                                <button type="button" className="btn btn-secondary" data-dismiss="modal">닫기</button>
                                <button type="submit" className={`btn ${added ? "btn-success" : "btn-primary"}`}>호스트
                                    추가
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    )
}

const AddStudentSection: React.FC<{ examId: string }> = ({examId}) => {
    const modalId = "addStudentModal"
    const [studentId, setStudentId] = useState<string | null>(null)
    const [added, setAdded] = useState(false)
    const {register, handleSubmit, reset} = useForm()

    const onSubmit = useCallback(async ({name}) => {
        const { data } = await NormioApi.addStudent(examId, name)
        const studentId = data[0].Fields.student.id
        setStudentId(studentId)
        setAdded(true)
        reset()
    }, [examId, setAdded])
    return (
        <div className={"card-text text-center p-2"}>
            <h5> 시험에 참여할 학생들을 등록하세요. </h5>
            <p className={"font-weight-light"}> 등록에 필요한 ID와 암호를 생성합니다. </p>
            <button className={"btn btn-light btn-block"} data-toggle="modal" data-target={`#${modalId}`}>네, 지금 등록할게요.
            </button>
            <div className="modal fade" id={modalId} tabIndex={-1} role="dialog" aria-labelledby={modalId}
                 aria-hidden="true">
                <div className="modal-dialog" role="document">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h5 className="modal-title" id="exampleModalLabel">참여 학생을 추가합니다</h5>
                            <button type="button" className="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <form onSubmit={handleSubmit(onSubmit)}>
                            <div className="modal-body">
                                <div className={"form-group"}>
                                    <input className={"form-control"} type="text" name={"name"} placeholder={"학생 이름"}
                                           ref={register}/>
                                </div>
                                {studentId && <div className={"text-center"}>
                                    <p>생성된 학생 아이디입니다.</p>
                                    <div className={"font-weight-light"}>
                                        {studentId}
                                    </div>
                                </div>}
                            </div>
                            <div className="modal-footer">
                                <button type="button" className="btn btn-secondary" data-dismiss="modal">닫기</button>
                                <button type="submit" className={`btn ${added ? "btn-success" : "btn-primary"}`}>학생 추가
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    )
}

const ChangeExamConfigSection: React.FC<{ examId: string, title: string }> = ({examId, title}) => {
    const modalId = "changeExamConfig"
    const [added, setAdded] = useState(false)
    const {register, handleSubmit} = useForm()

    const onSubmit = useCallback(async ({newTitle}) => {
        await NormioApi.changeExamTitle(examId, newTitle)
        setAdded(true)
    }, [examId, setAdded])

    return (
        <div className={"card-text text-center p-2"}>
            <h5> 앗, 뭔가 잘못 설정 하셨나요? </h5>
            <p className={"font-weight-light"}> 걱정마세요. 바로 변경할 수 있습니다. </p>
            <button className={"btn btn-light btn-block"} data-toggle="modal" data-target={`#${modalId}`}>네, 수정하고
                싶습니다.
            </button>
            <div className="modal fade" id={modalId} tabIndex={-1} role="dialog" aria-labelledby={modalId}
                 aria-hidden="true">
                <div className="modal-dialog" role="document">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h5 className="modal-title" id="exampleModalLabel">설정을 변경합니다</h5>
                            <button type="button" className="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <form onSubmit={handleSubmit(onSubmit)}>
                            <div className="modal-body">
                                <div className={"form-group"}>
                                    <input className={"form-control"} type="text" name={"newTitle"}
                                           placeholder={"새로운 시험 명"} ref={register}/>
                                </div>
                            </div>
                            <div className="modal-footer">
                                <button type="button" className="btn btn-secondary" data-dismiss="modal">닫기</button>
                                <button type="submit" className={`btn ${added ? "btn-success" : "btn-primary"}`}>설정 저장
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    )
}

const CreateExamPage: React.FC = props => {
    const [examId, setExamId] = useState("")
    const [title, setTitle] = useState("")
    const [hostId, setHostId] = useState<string>()
    const [examCreated, setExamCreated] = useState(false)

    const handleHostLogin = useCallback(() => {
        setHostId(nanoid(12))
    }, [])
    const handleCreateExam = useCallback(data => {
        setExamCreated(true)
        setExamId(data.examId)
        setTitle(data.title)
    }, [setExamCreated, setExamId])

    return (
        <div className={"container"}>
            <div className={"row"}>
                {!examCreated ? <div className={"col-3 my-5"}></div> : null}
                <div className={"col"}>
                    <div className={"row"}>
                        <div className={"col my-3"}>
                            <HostLogin hostId={hostId} handleSubmit={handleHostLogin}/>
                        </div>
                    </div>
                    <div className={"row"}>
                        <div className={"col my-3"}>
                            <CreateExamCard hostId={hostId} handleSubmitCreateExam={handleCreateExam}/>
                        </div>
                    </div>
                </div>
                {examCreated ?
                    <div className={"col my-3"}>
                        <div className={"card shadow-lg"}>
                            <div className={"card-header font-weight-bold text-center"}>
                                이제 무엇을 해야 하나요?
                            </div>
                            <div className={"card-body"}>
                                <AddHostSection examId={examId}/>
                                <AddStudentSection examId={examId}/>
                                <ChangeExamConfigSection examId={examId} title={title}/>
                                <div className={"card-text text-center p-2"}>
                                    <h5> 지금 바쁘신가요? </h5>
                                    <p className={"font-weight-light"}> 나중에 언제든지 돌아오세요. <br/> 등록된 계정에서 언제든지 다시 수정할 수
                                        있습니다. </p>
                                    <button className={"btn btn-light btn-block"}>네, 그럴게요.</button>
                                </div>
                            </div>
                        </div>
                    </div> : <div className={"col-3 my-5"}/>}
            </div>
        </div>
    )
}

export default CreateExamPage