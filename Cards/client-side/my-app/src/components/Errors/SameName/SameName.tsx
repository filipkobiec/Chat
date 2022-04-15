import { Button } from 'react-bootstrap';
import styles from './SameName.module.scss'; 


function SameName(){
    return (
        <div className={styles.errorContainer}>
            <div className={styles.errorMessage}>
                <h2>There was a problem with server connection please try again</h2>
                <Button href='/'>Reload</Button>
            </div>
        </div>
    )
}

export default SameName;