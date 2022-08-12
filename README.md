# VitalStats
Simple packet of code that allows you to add vital stats to your game by several lines fo code.

#Usage

You can add 'EntityStats' component to have a stats storage or you can directly create 'Stat' field and work with it. 
One of the main advantages of this is value sequrity. For example, in 'RangedStat' it cannot be less 0 or bigger than max value that you can setup. 
So you don't need to worry about data safety and don't be aware that health will be -5 :) 'RangedStat' code just set this to zero.

The other one is SimpleStatRule (maybe I will add other, more complicated rules, but simple rule is also a good one). 
It defines the delta time and the value the stat value will be increased by, and it goes automatically. If you have a
reference to this rule from your code, you can easily change delta time or value, stop or restart this rule at any time.
It's a good thing when you want to add constant starve increasing, it just could be done by 1 line!
