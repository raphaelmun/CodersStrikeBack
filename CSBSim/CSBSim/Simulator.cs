using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class SimulatorPodState
{
    public int X { get; set; }
    public int Y { get; set; }
    public int vX { get; set; }
    public int vY { get; set; }
    public int Angle { get; set; }
    public int Shield { get; set; }
    public int Boost { get; set; }

    public static SimulatorPodState Parse( string input, bool skipFirst = true )
    {
        var inputs = input.Split( ' ' );
        var state = new SimulatorPodState();
        int startIndex = skipFirst ? 1 : 0;
        state.X = int.Parse( inputs[ startIndex + 0 ] ); // x position of your pod
        state.Y = int.Parse( inputs[ startIndex + 1 ] ); // y position of your pod
        state.vX = int.Parse( inputs[ startIndex + 2 ] ); // x speed of your pod
        state.vY = int.Parse( inputs[ startIndex + 3 ] ); // y speed of your pod
        state.Angle = int.Parse( inputs[ startIndex + 4 ] ); // angle of your pod
        state.Shield = int.Parse( inputs[ startIndex + 5 ] ); // shield count (0-3)
        state.Boost = int.Parse( inputs[ startIndex + 6 ] ); // boost (0/1) 1 if used
        return state;
    }

    public int Comparison( string input )
    {
        var state = SimulatorPodState.Parse( input, false );
        int total = 0;
        total += Math.Abs( X - state.X );
        total += Math.Abs( Y - state.Y );
        total += Math.Abs( vX - state.vX );
        total += Math.Abs( vY - state.vY );
        total += Math.Abs( Angle - state.Angle );
        total += Math.Abs( Shield - state.Shield );
        total += Math.Abs( Boost - state.Boost );
        return total;
    }

    public override string ToString()
    {
        return string.Format( "{0} {1} {2} {3} {4} {5} {6}", X, Y, vX, vY, Angle, Shield, Boost );
    }
}

public enum TagCategories : int
{
    NONE = 0x000,
    MOVE_ONLY = 0x001,
    BOOST = 0x002,
    BOOST_DEPLETED = 0x004,
    SHIELD = 0x008,
    ENGINE_COOLDOWN = 0x010,
    SINGLE_COLLISION = 0x020,
    MULTI_COLLISION = 0x040,
    ONLY_HIGH_IMPACT = 0x080,
    HAS_LOW_IMPACT = 0x100,
    OTHER = 0x200,
    ALL = 0xFFF,
}

public class SimulatorMove
{
    public TagCategories Tags { get; set; }
    public SimulatorPodState In { get; set; }
    public SimulatorPodState Out { get; set; }
    public int tX { get; set; }
    public int tY { get; set; }
    public string Thrust { get; set; }

    public override string ToString()
    {
        return string.Format( "IN {0}\nMOVE {1} {2} {3}\nOUT {4}", In, tX, tY, Thrust, Out );
    }
}

public class Simulator
{
    List<SimulatorMove[]> moves;

    public List<SimulatorMove[]> Moves
    {
        get
        {
            return moves;
        }
    }

    public Simulator()
    {
        moves = new List<SimulatorMove[]>();
    }
    
    public List<SimulatorMove[]> GetMoves( TagCategories tags )
    {
        return moves.Where( x => ( x[ 0 ].Tags & tags ) > 0 ).ToList();
    }

    public bool Load( string dataFile )
    {
        moves.Clear();
        if( File.Exists( dataFile ) )
        {
            var lines = File.ReadAllLines( dataFile );
            for( int i = 0; i < lines.Length; i++ )
            {
                if( lines[ i ].StartsWith( "tags:" ) )
                {
                    SimulatorMove[] move = new SimulatorMove[ 4 ];
                    for( int j = 0; j < 4; j++ )
                    {
                        move[ j ] = new SimulatorMove();
                    }
                    TagCategories fullTags = TagCategories.NONE;
                    var tags = lines[ i ].Split( ' ' );
                    for( int t = 1; t < tags.Length; t++ )
                    {
                        TagCategories tag = (TagCategories)Enum.Parse( typeof( TagCategories ), tags[ t ] );
                        fullTags = (TagCategories)( (uint)fullTags | (uint)tag );
                    }

                    // read the next 4 lines as IN
                    for( int j = 0; j < 4; j++ )
                    {
                        move[ j ].Tags = fullTags;
                        move[ j ].In = SimulatorPodState.Parse( lines[ i + j + 1 ] );
                    }
                    // read the next 4 lines as MOVE
                    for( int j = 0; j < 4; j++ )
                    {
                        var inputs = lines[ i + j + 5 ].Split( ' ' );
                        move[ j ].tX = int.Parse( inputs[ 1 ] ); // target x
                        move[ j ].tY = int.Parse( inputs[ 2 ] ); // target y
                        move[ j ].Thrust = inputs[ 3 ]; // thrust
                    }
                    // read the next 4 liens as OUT
                    for( int j = 0; j < 4; j++ )
                    {
                        move[ j ].Out = SimulatorPodState.Parse( lines[ i + j + 9 ] );
                    }
                    moves.Add( move );
                    i += 12;
                }
            }
            return true;
        }
        return false;
    }
}