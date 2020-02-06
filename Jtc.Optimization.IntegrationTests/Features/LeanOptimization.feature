Feature: LeanOptimization

Scenario Outline: Optimize parameters
	Given I have an optimization.config
	And I have set maxThreads and generations to <maxThreads>
	And I have set useSharedAppDomain to <useSharedAppDomain>
	When I optimize
	Then the Sharp Ratio should be <sharpRatio>
	And Total Trades should be <totalTrades>
	And last run should produce different result
	And multiple threads should execute in parallel

	Examples:
		| maxThreads | useSharedAppDomain | sharpRatio | totalTrades |
		| 1          | true               | 4.8242     | 1           |
		| 2          | true               | 4.8242     | 1           |
		| 1          | false              | 4.8242     | 1           |
		| 2          | false              | 4.8242     | 1           |
		| 7          | false              | 4.8242     | 1           |
		| 7          | true               | 4.8242     | 1           |

#Scenario Outline: Optimize parameters
#	Given I have an optimization.config
#	And I have set maxThreads and generations to <maxThreads>
#	And I have set useSharedAppDomain to <useSharedAppDomain>
#	And I have set Fitness to WalkForwardSharpeMaximizer with folds <folds>
#	When I optimize
#	Then the Sharp Ratio should be <sharpRatio>
#	And Total Trades should be <totalTrades>
#	And last run should produce different result
#	And multiple threads should execute in parallel
#
#	Examples:
#		| maxThreads | useSharedAppDomain | sharpRatio | totalTrades | folds |
#		| 1          | true               | 4.8242     | 1           | 2     |
#		| 2          | true               | 4.8242     | 1           | 2     |
#		| 1          | false              | 4.8242     | 1           | 2     |
#		| 2          | false              | 4.8242     | 1           | 2     |
#		| 1          | true               | 4.8242     | 1           | 3     |
#		| 2          | true               | 4.8242     | 1           | 3     |
#		| 1          | false              | 4.8242     | 1           | 3     |
#		| 2          | false              | 4.8242     | 1           | 3     |