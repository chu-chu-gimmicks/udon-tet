namespace ChuChuGimmicks.UDONTET
{
    public enum PlayerAction : int
    {
        None        = 0,
        Move        = 1,
        Spin        = 2,
        SoftDrop    = 4,
        HardDrop    = 8,
        FirstHold   = 16,
        Hold        = 32,
        ChairAdjust = 64,
        Pause       = 128,
        Guide       = 256
    }
}
