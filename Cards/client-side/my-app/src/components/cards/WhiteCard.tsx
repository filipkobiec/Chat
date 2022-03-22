import styles from './WhiteCard.module.scss'; 


function WhiteCard({text} : {text : string}) {
    return(
        <div className={styles.card}>
            <h2>{text}</h2>
        </div>
    )
}

export default WhiteCard