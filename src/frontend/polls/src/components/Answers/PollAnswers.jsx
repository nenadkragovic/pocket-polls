import React from 'react';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import { Bar } from "react-chartjs-2";

export function PollAnswers (props) {
    console.log(props.answers)
    return (
        <>
            {
                props.poll == null ?
                <h4>Selet poll to view data</h4> :
                <>
                     <Typography>{props.poll.name}</Typography>
                     {
                        props.answers.yesNoAnswers.map((item) => (
                            <Box>
                                <p>{item.QuestionText}</p>
                            </Box>
                        ))
                     }
                </>
            }
        </>
    )
}