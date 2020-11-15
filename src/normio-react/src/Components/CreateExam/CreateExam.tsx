import React, {useCallback, useState} from "react";

type CreateExamCardProps = {
    handleSubmit: () => void,
}

const CreateExamCard: React.FC<CreateExamCardProps> = props => {
    const [created, setCreated] = useState(false);
    const onSubmit = useCallback(event => {
        event.preventDefault()
        setCreated(true)
        props.handleSubmit()
    }, [])
    return (
        <div className={"card shadow-lg"}>
            <div className={"card-header font-weight-bold"}>
                시험 구성하기
            </div>
            <div className={"card-text p-3"}>
                <form onSubmit={onSubmit}>
                    <div className={"form-group"}>
                        <input className={"form-control"} placeholder={"Host ID"} />
                    </div>
                    <div className={"form-group"}>
                        <input className={"form-control"} placeholder={"시험 명"} />
                    </div>
                    <div className={"form-group"}>
                        <input className={"form-control"} placeholder={"시험 시작 시간"} />
                    </div>
                    <input type="submit" className={`btn btn-${ created ? "success"  : "primary"} btn-block`} value={`${created ? "시험이 생성 되었습니다!"  : "시험을 생성합니다"}`} />
                </form>
            </div>
        </div>
    )
}

export default CreateExamCard