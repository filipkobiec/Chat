import {  Modal } from 'react-bootstrap';


type SetMessageFunction = (message: string) => void

function ErrorModal({message, setMessage} : {message : string, setMessage : SetMessageFunction}){
    return (
        <Modal show={true} onHide={() => {setMessage("")}}>
            <Modal.Header>
            <Modal.Title>Error</Modal.Title>
            </Modal.Header>
            <Modal.Body>{message}</Modal.Body>
      </Modal>
    )
}


export default ErrorModal;