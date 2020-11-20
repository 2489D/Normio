import React, {useCallback, useContext} from "react";
import {useForm} from "react-hook-form";

const PaperSubmission:React.FC = props => {
    const { register, handleSubmit } = useForm()

    const onSubmit = (data: any) => {
        console.debug(data)
        const formData = new FormData();
        formData.append("submission", data.submission[0])
        console.debug(formData)
    }

    return (
        <form className={"form-group"} onSubmit={handleSubmit(onSubmit)}>
            <div className="input-group">
                <div className="custom-file">
                    <input id="inputGroupFile04" type="file" ref={register} name="submission" />
                    <label htmlFor="inputGroupFile04">답안 선택</label>
                    <button className="btn btn-outline-secondary" type="submit" id="inputGroupFileAddon04">제출</button>
                </div>
            </div>
        </form>
    )
}

export default PaperSubmission