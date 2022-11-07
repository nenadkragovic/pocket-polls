import React from 'react';
import { useState, useEffect } from 'react';
import  * as style from './style/answers.scss';
import * as http from '../../scripts/http';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import TablePagination from '@mui/material/TablePagination';
import Paper from '@mui/material/Paper';
import { Container } from '@mui/material';
import Button from '@mui/material/Button';
import DeleteForeverIcon from '@mui/icons-material/DeleteForever';
import * as validation from '../../scripts/helper';
import Snackbar from '@mui/material/Snackbar';
import MuiAlert from '@mui/material/Alert';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogTitle from '@mui/material/DialogTitle';
import { PollAnswers } from './PollAnswers';

function Answers() {
    const initData = {
        items: [],
        totalRecords: 0
    }

    const [polls, setPolls] = React.useState(initData);
    const [rowsPerPage, setRowsPerPage] = React.useState(5);
    const [page, setPage] = React.useState(0);

    const [validationData, setValidationData] = React.useState({
        open: false,
        message: '',
    });

    const [open, setOpen] = React.useState(false);
    const [pollToDelete, setPollToDelete] = React.useState(null);
    const [selectedPoll, setSelectedPoll] = React.useState(null);
    const [selectedPollAnswers, setSelectedPollAnswers] = React.useState(null);

    useEffect(() => {
        fetchData();
    },[]);

    const handleCloseValidationMessage = () => {
        setValidationData({
            open: false,
            message: '',
        })
    }

    const handleOpenDeleteDialog = (pollId) => {
        setOpen(true);
        setPollToDelete(pollId);
    };

    const handleClose = () => {
        setOpen(false);
        setPollToDelete(null);
    };

    const fetchData = async (searchParam = '') => {
        let offset = rowsPerPage * page;
            await http.request("polls?getForUser=true&offset=" + offset + "&limit=" + rowsPerPage + "&searchParam=" + searchParam, 'GET', null)
                .then(result => {
                    setPolls({
                        items: result.data.records,
                        totalRecords: result.data.totalRecords
                    });
                }).catch(err => {
                    console.log(err);
                });

    };

    const searchPolls = async (event) => {
        setPage(0);
        await fetchData(event.target.value);
    }

    const handleChange = async (e, p) => {
        setPage(p);
        await fetchData();
    }

    const handleChangeRowsPerPage = async (event) => {
        setRowsPerPage(parseInt(event.target.value, 10));
        setPage(0);
        await fetchData();
    }

    const deletePoll = async () => {
        handleClose();
        await http.request(`polls/${pollToDelete}`, 'DELETE', null)
            .then(result => {
                setValidationData({
                    open: true,
                    message: 'Poll deleted successfully!',
                    severity: 'success'
                })
                var items = polls.items.filter(function(value, index, arr){
                    return value.id !== pollToDelete;
                });
                setPolls({
                    ...polls,
                    items: items,
                    totalRecords: polls.totalRecords - 1
                });
                fetchData()
            }).catch(err => {
                var message = validation.getValidationMessage(err.response.data);

                setValidationData({
                    open: true,
                    message: message,
                    severity: 'error'
                })
            });
    }

    useEffect(() => {
        if (selectedPoll != null)
            http.request(`answers/${selectedPoll.id}`)
                .then(result => {
                    setSelectedPollAnswers(result.data);
                }).catch(err => {
                    console.log(err);
                    setValidationData({
                        open: true,
                        message: 'Failed to fetch polls fata.',
                        severity: 'error'
                    })
                });
    },[selectedPoll]);

    return (
        <>
            <Container style={style} className="cont">
                <TableContainer component={Paper} style={{marginTop: '1rem'}}>
                <Table sx={{ minWidth: '100%' }} aria-label="simple table">
                    <TableHead>
                    <TableRow>
                        <TableCell align="left">Id</TableCell>
                        <TableCell>Name</TableCell>
                        <TableCell align="right"></TableCell>
                        <TableCell align="right"></TableCell>
                    </TableRow>
                    </TableHead>
                    <TableBody>
                    {polls.items.map((row) => (
                        <TableRow
                        key={row.id}
                        sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
                        >
                        <TableCell component="th" align="left" scope="row">
                            {row.id}
                        </TableCell>
                        <TableCell>{row.name}</TableCell>
                        <TableCell align="right">
                            <Button variant="outlined" onClick={()=> setSelectedPoll(row)}>show</Button>
                        </TableCell>
                        <TableCell align="right">
                            <Button variant="outlined" onClick={()=> handleOpenDeleteDialog(row.id)}><DeleteForeverIcon></DeleteForeverIcon></Button>
                        </TableCell>
                        </TableRow>
                    ))}
                    </TableBody>
                </Table>
                </TableContainer>
                <TablePagination
                    rowsPerPageOptions={[2, 5, 10, 25]}
                    component="div"
                    count={polls.totalRecords}
                    rowsPerPage={rowsPerPage}
                    page={page}
                    onPageChange={handleChange}
                    onRowsPerPageChange={handleChangeRowsPerPage}
                />
            <PollAnswers poll={selectedPoll} answers={selectedPollAnswers} style={style}></PollAnswers>
            </Container>
            <Snackbar
                className='validation'
                open={validationData.open}
                autoHideDuration={6000}
                onClose={handleCloseValidationMessage}>
                    <MuiAlert severity={validationData.severity} elevation={6} variant="filled" onClose={handleCloseValidationMessage}>
                        {validationData.message}
                    </MuiAlert>
            </Snackbar>
            <Dialog
                open={open}
                onClose={handleClose}
                aria-labelledby="alert-dialog-title"
                aria-describedby="alert-dialog-description"
                >
                <DialogTitle id="alert-dialog-title">
                {"Are you sure?"}
                </DialogTitle>
                <DialogActions>
                <Button onClick={deletePoll}>Yes</Button>
                <Button onClick={handleClose} autoFocus>
                    No
                </Button>
                </DialogActions>
            </Dialog>
        </>
    );
};

export default Answers;
