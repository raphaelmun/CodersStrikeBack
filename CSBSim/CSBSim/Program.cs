using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

class Player
{
    static void Main( string[] args )
    {
        Simulator simulator = new Simulator();
        //simulator.Load( "data/replay_vsdephille.txt" );
        //simulator.Load( "data/replay_vsjeff.txt" );
        simulator.Load( "data/replay_vsmagus_1.txt" );

        Console.WriteLine( "Loaded Replay" );
        var moves = simulator.GetMoves( TagCategories.SINGLE_COLLISION );
        for( int m = 0; m < moves.Count; m++ )
        {
            var move = moves[ m ];
            for( int i = 0; i < 4; i++ )
            {
                string sim = move[ i ].Out.ToString();
                //string pod = pod[ i ].SimulatorString(); // Output: "X Y vX vY Angle Shield Boost"
                string pod = move[ i ].Out.ToString();
                int compareValue = move[ i ].Out.Comparison( pod );
                if( compareValue > 10 )
                {
                    //pod[ i ].Step( 1 );
                }
                Console.WriteLine( "{0}: {1} E:{2} vs. A:{3}", m, compareValue == 0 ? " " : compareValue.ToString(), pod, sim );
            }
        }
        Console.ReadKey();
    }
}