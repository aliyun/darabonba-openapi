<?php

declare(strict_types=1);

/*
 * This file is part of PHP CS Fixer.
 *
 * (c) Fabien Potencier <fabien@symfony.com>
 *     Dariusz Rumiński <dariusz.ruminski@gmail.com>
 *
 * This source file is subject to the MIT license that is bundled
 * with this source code in the file LICENSE.
 */

namespace Darabonba\OpenApi\Exceptions;

use AlibabaCloud\Dara\Exception\DaraException;

class ServerException extends DaraException
{
    public function __construct($map)
    {
        parent::__construct($map);
    }
}
