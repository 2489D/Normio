import React from 'react'
import { Link } from 'react-router-dom'
import CurrentTime from '../Components/CurrentTime'

type HomeCardProps = {
    title: string,
    action?: string,
    actionRoute?: string
}

const Card: React.FC<HomeCardProps> = props => {
    return (
        <div className="card shadow-lg m-2" style={{ width: "18rem" }}>
            <div className="card-header d-flex">
                <span className="font-weight-bold mx-auto">
                    {props.title}
                </span>
            </div>
            <div className="card-body">
                <p className="card-text">
                    {props.children}
                </p>
                {props.action && props.actionRoute
                    ? 
                    <div className={"my-2"}>
                        <Link to={props.actionRoute}>
                            <button className="btn btn-light btn-block">{props.action}</button>
                        </Link>
                    </div>
                    : null}
                <div className={"my-2"}>
                    <a href="#" className="btn btn-sm btn-info btn-block">방법 살펴보기</a>
                </div>
            </div>
        </div>
    )
}

const Home: React.FC = props => {
    return (
        <div className="container">
            <div className="row">
                <div className="col">
                    <div className="row my-3">
                        <div className="col d-flex">
                            <h1 className="font-weight-bold mx-auto">Welcome to Normio!</h1>
                        </div>
                    </div>
                    <div className="row my-1">
                        <div className="col d-flex">
                            <h3 className="mx-auto">온라인 시험 플랫폼</h3>
                        </div>
                    </div>
                    <div className="row">
                        <div className="col">
                            <div className="row d-flex">
                                <p className="font-weight-light mx-auto">출석체크, 시험문제 관리, 제출된 시험지 관리, 시험 운영, 화상 채팅, 부정 행위 탐지...</p>
                            </div>
                            <div className="row d-flex">
                                <p className="font-weight-light mx-auto">이제는 모두 이곳 norm.io에서</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div className="row">
                <div className="col d-flex">
                    <div className="mx-auto">
                        <CurrentTime variant="h2" />
                    </div>
                </div>
            </div>
            <div className="row">
                <div className="col">
                    <Card title="시험을 진행하시나요?" action="시험을 구성합니다" actionRoute="/exams/create">
                        <p>시험을 구성하고, 진행하는 방법을 살펴보세요.</p>
                        <p>시험을 운영하는 데 필요한 모든 방법에 대해 설명해드립니다.</p>
                    </Card>
                </div>
                <div className="col">
                    <Card title="시험에 참여하시나요?" action="시험에 참여합니다" actionRoute="/exams">
                        <p>시험에 참여하시나요?</p>
                        <p>시험에 참여하는 방법과 시험이 어떻게 진행되는지 살펴보세요.</p>
                    </Card>
                </div>
                <div className="col">
                    <Card title="무엇을 할 수 있나요?" action="문의하기">
                        <p>더 궁금한 점이 있으신가요?</p>
                        <p>자세히 보기 버튼을 눌러 Normio가 제공하는 모든 서비스를 살펴보세요.</p>
                    </Card>
                </div>
            </div>
        </div>
    )
}

export default Home