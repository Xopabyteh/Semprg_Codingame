using System;

public class TCP
{
    public static string TraverseStates(string[] events)
    {
        var state = "CLOSED"; // Initial state, always

        for (int i = 0; i < events.Length; i++)
        {
            //(the format is INITIAL_STATE: EVENT -> NEW_STATE)
            //CLOSED: APP_PASSIVE_OPEN -> LISTEN
            //CLOSED: APP_ACTIVE_OPEN  -> SYN_SENT
            //LISTEN: RCV_SYN          -> SYN_RCVD
            //LISTEN: APP_SEND         -> SYN_SENT
            //LISTEN: APP_CLOSE        -> CLOSED
            //SYN_RCVD: APP_CLOSE      -> FIN_WAIT_1
            //SYN_RCVD: RCV_ACK        -> ESTABLISHED
            //SYN_SENT: RCV_SYN        -> SYN_RCVD
            //SYN_SENT: RCV_SYN_ACK    -> ESTABLISHED
            //SYN_SENT: APP_CLOSE      -> CLOSED
            //ESTABLISHED: APP_CLOSE   -> FIN_WAIT_1
            //ESTABLISHED: RCV_FIN     -> CLOSE_WAIT
            //FIN_WAIT_1: RCV_FIN      -> CLOSING
            //FIN_WAIT_1: RCV_FIN_ACK  -> TIME_WAIT
            //FIN_WAIT_1: RCV_ACK      -> FIN_WAIT_2
            //CLOSING: RCV_ACK         -> TIME_WAIT
            //FIN_WAIT_2: RCV_FIN      -> TIME_WAIT
            //TIME_WAIT: APP_TIMEOUT   -> CLOSED
            //CLOSE_WAIT: APP_CLOSE    -> LAST_ACK
            //LAST_ACK: RCV_ACK        -> CLOSED

            var tcpEvent = events[i];
            state = (state, tcpEvent) switch
            {
                (TCPState.Closed, TCPEvent.AppPassiveOpen) => TCPState.Listen,
                (TCPState.Closed, TCPEvent.AppActiveOpen) => TCPState.SynSent,
                (TCPState.Listen, TCPEvent.RcvSyn) => TCPState.SynReceived,
                (TCPState.Listen, TCPEvent.AppSend) => TCPState.SynSent,
                (TCPState.Listen, TCPEvent.AppClose) => TCPState.Closed,
                (TCPState.SynReceived, TCPEvent.AppClose) => TCPState.FinWait1,
                (TCPState.SynReceived, TCPEvent.RcvAck) => TCPState.Established,
                (TCPState.SynSent, TCPEvent.RcvSyn) => TCPState.SynReceived,
                (TCPState.SynSent, TCPEvent.RcvSynAck) => TCPState.Established,
                (TCPState.SynSent, TCPEvent.AppClose) => TCPState.Closed,
                (TCPState.Established, TCPEvent.AppClose) => TCPState.FinWait1,
                (TCPState.Established, TCPEvent.RcvFin) => TCPState.CloseWait,
                (TCPState.FinWait1, TCPEvent.RcvFin) => TCPState.Closing,
                (TCPState.FinWait1, TCPEvent.RcvFinAck) => TCPState.TimeWait,
                (TCPState.FinWait1, TCPEvent.RcvAck) => TCPState.FinWait2,
                (TCPState.Closing, TCPEvent.RcvAck) => TCPState.TimeWait,
                (TCPState.FinWait2, TCPEvent.RcvFin) => TCPState.TimeWait,
                (TCPState.TimeWait, TCPEvent.AppTimeout) => TCPState.Closed,
                (TCPState.CloseWait, TCPEvent.AppClose) => TCPState.LastAck,
                (TCPState.LastAck, TCPEvent.RcvAck) => TCPState.Closed,
                _ => TCPState.Error
            };

            if(state == TCPState.Error)
                return state;
        }

        return state;
    }


    public static class TCPState
    {
        //CLOSED, LISTEN, SYN_SENT, SYN_RCVD, ESTABLISHED, CLOSE_WAIT, LAST_ACK, FIN_WAIT_1, FIN_WAIT_2, CLOSING, TIME_WAIT
        public const string Closed = "CLOSED";
        public const string Listen = "LISTEN";
        public const string SynSent = "SYN_SENT";
        public const string SynReceived = "SYN_RCVD";
        public const string Established = "ESTABLISHED";
        public const string CloseWait = "CLOSE_WAIT";
        public const string LastAck = "LAST_ACK";
        public const string FinWait1 = "FIN_WAIT_1";
        public const string FinWait2 = "FIN_WAIT_2";
        public const string Closing = "CLOSING";
        public const string TimeWait = "TIME_WAIT";
        public const string Error = "ERROR";
    }

    public static class TCPEvent
    {
        //APP_PASSIVE_OPEN, APP_ACTIVE_OPEN, APP_SEND, APP_CLOSE, APP_TIMEOUT, RCV_SYN, RCV_ACK, RCV_SYN_ACK, RCV_FIN, RCV_FIN_ACK
        public const string AppPassiveOpen = "APP_PASSIVE_OPEN";
        public const string AppActiveOpen = "APP_ACTIVE_OPEN";
        public const string AppSend = "APP_SEND";
        public const string AppClose = "APP_CLOSE";
        public const string AppTimeout = "APP_TIMEOUT";
        public const string RcvSyn = "RCV_SYN";
        public const string RcvAck = "RCV_ACK";
        public const string RcvSynAck = "RCV_SYN_ACK";
        public const string RcvFin = "RCV_FIN";
        public const string RcvFinAck = "RCV_FIN_ACK";
    }
}